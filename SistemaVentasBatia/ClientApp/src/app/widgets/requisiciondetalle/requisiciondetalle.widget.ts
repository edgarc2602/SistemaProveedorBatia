import { Component, OnChanges, Output, EventEmitter, Inject, ViewChild, ElementRef, SimpleChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RequisicionDetalle } from '../../models/requisicionDetalle';
import { RequisicionProducto } from '../../models/requisicionProducto';
import { Resultado } from '../../models/resultado';
import Swal from 'sweetalert2';
import { Requisicion } from '../../models/requisicion';
import { StoreUser } from '../../stores/StoreUser';
import { User } from 'oidc-client';
declare var bootstrap: any;

@Component({
    selector: 'requisicionDetalle-widget',
    templateUrl: './requisicionDetalle.widget.html'
})
export class RequisicionDetalleWidget {
    @Output('supEvent') sendEvent = new EventEmitter<boolean>();

    modelRequisicion: Requisicion = {
        idRequisicion: 0,
        idProveedor: 0,
        idOrdenCompra: 0,
        comprador: "",
        comentarios: "",
        fechaAlta: "",
        idEstatus: 0,
        estatus: "",
        iva: 0,
        ivaPorcentaje: 0,
        subtotal: 0,
        total: 0,
        ivaNuevo: 0,
        subtotalNuevo: 0,
        totalNuevo: 0
    };

    model: RequisicionDetalle = {
        idRequisicion: 0,
        productos: []
    };

    loading: boolean = false;
    isLoading: boolean = false;
    idRequisicion: number = 0;
    idRequisicionEstatus: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) { }

    ngOnInit() {
        this.limpiaModelo();
    }

    cargaRequisicionDetalle(): void {
        this.isLoading = true;
        this.http.get<RequisicionDetalle>(`${this.url}api/requisicion/getrequisiciondetalle/${this.idRequisicion}`).subscribe(response => {
            this.model = response;
            this.isLoading = false;
            console.log(this.model);
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'Ocurrio un error al consultar el detalle de la requisicion',
                icon: 'error',
                timer: 3000,
                showConfirmButton: false,
            });
        });
    }

    guardaPreciosNuevos(): void {
        if (this.validaPreciosNuevos()) {
            Swal.fire({
                title: '\u00BFEstas seguro?',
                text: 'Se actualizaran los precios nuevos de la requisicion y se enviara una notificacion al comprador asignado.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Si, guardar',
                cancelButtonText: 'Cancelar',
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.isLoading = true;
                    
                    this.calculaTotales();

                    // Se encarga de actualizar los precios de la requisicion por el proveedor
                    this.http.put<Resultado>(`${this.url}api/requisicion/putactualizarequisicionnuevoprecio/${this.modelRequisicion.ivaNuevo}/${this.modelRequisicion.subtotalNuevo}/${this.modelRequisicion.totalNuevo}/${this.modelRequisicion.idProveedor}`, this.model)
                        .subscribe(response => {
                            console.log(response);
                            this.isLoading = false;
                            this.cargaRequisicionDetalle();
                            Swal.fire({
                                title: '\u00A1Exito!',
                                text: response.mensaje,
                                icon: 'success',
                                timer: 2500,
                                showConfirmButton: false
                            });

                            this.enviarCorreoComprador(this.modelRequisicion, this.user.nombre);
                        }, err => {
                            console.error(err);
                            this.isLoading = false;
                            Swal.fire({
                                title: 'Error',
                                text: 'Ocurrio un error al consultar el detalle de la requisicion.',
                                icon: 'error',
                                timer: 3000,
                                showConfirmButton: false
                            });
                        });
                } else {
                    this.isLoading = false;
                }
            });
        }
    }

    enviarCorreoComprador(requisicion: Requisicion, proveedor: string) {
        this.http.post<Resultado>(`${this.url}api/correo/enviarcorreocomprador/${proveedor}`, requisicion)
            .subscribe(response => {
                this.cargaRequisicionDetalle();
                //Swal.fire({
                //    title: 'Correo enviado',
                //    text: response.mensaje,
                //    icon: 'success',
                //    timer: 2500,
                //    showConfirmButton: false
                //});
            },
                err => {
                    console.error(err);
                    this.isLoading = false;
                    //Swal.fire({
                    //    title: 'Error',
                    //    text: 'Ocurrio un error al enviar el correo al comprador.',
                    //    icon: 'error',
                    //    timer: 3000,
                    //    showConfirmButton: false
                    //});
                });
    }

    calculaTotales() {
        let ivaPorcentaje: number = this.modelRequisicion.ivaPorcentaje;
        let ivaNuevo: number = 0;
        let subTotalNuevo: number = 0;
        let totalNuevo: number = 0;

        // Acumula el total de productos * precio por unidad
        this.model.productos.forEach((producto, index) => {

            if (producto.precioNuevo > 0) {
                subTotalNuevo += producto.cantidad * producto.precioNuevo;
            }
            else {
                subTotalNuevo += producto.cantidad * producto.precio;
            }
        });

        ivaNuevo = subTotalNuevo * ivaPorcentaje;
        totalNuevo = subTotalNuevo + ivaNuevo;

        this.modelRequisicion.ivaNuevo = ivaNuevo;
        this.modelRequisicion.subtotalNuevo = subTotalNuevo;
        this.modelRequisicion.totalNuevo = totalNuevo;
    }

    validaPreciosNuevos() {
        this.isLoading = true;

        let precioNuevoInvalido = true;
        for (let x = 0; x < this.model.productos.length; x++) {
            let precioNuevo: number = this.model.productos[x].precioNuevo;
            if (!/^\d+(\.\d+)?$/.test(precioNuevo.toString())) {
                precioNuevoInvalido = false;
                break;
            }
        }

        if (!precioNuevoInvalido) {
            Swal.fire({
                title: "Advertencia",
                text: "Debe agregar solo precios enteros o decimales al nuevo precio.",
                icon: "warning"
            });
            this.isLoading = false;
            return false;
        }

        let cambioPrecio: boolean = false;
        for (let x = 0; x < this.model.productos.length; x++) {
            let precioNuevo: number = this.model.productos[x].precioNuevo;
            if (precioNuevo > 0) {
                cambioPrecio = true;
                break;
            }
        }

        if (!cambioPrecio) {
            Swal.fire({
                title: "Advertencia",
                text: "Debe agregar un nuevo precio a un producto",
                icon: "warning"
            });
            this.isLoading = false;
            return false;
        }

        return true;
    }

    limpiaModelo() {
        this.model = {
            idRequisicion: 0,
            productos: []
        };

        this.modelRequisicion = {
            idRequisicion: 0,
            idProveedor: 0,
            idOrdenCompra: 0,
            comprador: "",
            comentarios: "",
            fechaAlta: "",
            idEstatus: 0,
            estatus: "",
            iva: 0,
            ivaPorcentaje: 0,
            subtotal: 0,
            total: 0,
            ivaNuevo: 0,
            subtotalNuevo: 0,
            totalNuevo: 0
        };
    }

    regresaSup() {

    }

    open(requisicion: Requisicion): void {
        let docModal = document.getElementById('modalRequisicionDetalle');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        this.modelRequisicion = requisicion;
        this.idRequisicion = requisicion.idRequisicion;
        this.idRequisicionEstatus = requisicion.idEstatus;
        this.cargaRequisicionDetalle();
        myModal.show();
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.sendEvent.emit(false);
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalPresentacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }

    soloNumeros(event: any, index: number): void {
        const input = event.target as HTMLInputElement;

        if (input.value == '') {
            input.value = '0';
        }

        input.value = input.value
            .replace(/[^0-9.]/g, '')   // elimina todo lo que no sea número o punto
            .replace(/(\..*)\./g, '$1'); // evita más de un punto decimal
        this.model.productos[index].precioNuevo = parseFloat(input.value); // actualiza el modelo si usas ngModel
    }
}