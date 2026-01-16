import { Component, OnChanges, Output, EventEmitter, Inject, ViewChild, ElementRef, SimpleChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Resultado } from '../../models/resultado';
import Swal from 'sweetalert2';
import { StoreUser } from '../../stores/StoreUser';
import { User } from 'oidc-client';
import { OrdenCompraDetalle } from '../../models/ordenCompraDetalle';
import { OrdenCompra } from '../../models/ordencompra';
import { FacturaCFDI } from '../../models/FacturaCFDI';
import { RequisicionOrdenCompraDetalleFacturaWidget } from '../../widgets/requisicionordencompradetallefactura/requisicionordencompradetallefactura.widget';
import { parseString } from 'xml2js';
declare var bootstrap: any;

@Component({
    selector: 'requisicionOrdenCompraDetalle-widget',
    templateUrl: './requisicionOrdenCompraDetalle.widget.html'
})
export class RequisicionOrdenCompraDetalleWidget {
    @Output('supEvent') sendEvent = new EventEmitter<boolean>();

    @ViewChild(RequisicionOrdenCompraDetalleFacturaWidget, { static: false }) ordenCompraFacutura: RequisicionOrdenCompraDetalleFacturaWidget;

    idOrdenCompra: number = 0;

    modelOrdenCompra: OrdenCompra = {
        idOrden: 0,
        tipo: "",
        estatus: "",
        fechaAlta: "",
        empresa: "",
        proveedor: "",
        idCliente: 0,
        cliente: "",
        elabora: "",
        observacion: "",
        inventario: 0,
        facturado: 0,
        idCredito: 0,
        credito: "",
        diasCredito: 0,
        idAlmacen: 0,
        loading: false,
        iva: 0,
        subTotal: 0,
        total: 0
    };

    modelOrdenCompraDetalle: OrdenCompraDetalle = {
        idOrden: 0,
        productos: []
    };

    modelResultado: Resultado = {
        estatus: false,
        mensaje: "",
        mensajeError: "",
        objeto: "",
        objetos: []
    }

    loading: boolean = false;
    isLoading: boolean = false;
    idRequisicion: number = 0;
    idRequisicionEstatus: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) { }

    ngOnInit() {
        this.limpiaModelo();
    }

    cargaOrdenCompra(): void {
        this.isLoading = true;
        this.http.get<Resultado>(`${this.url}api/ordenCompra/getordencompra/${this.idOrdenCompra}`).subscribe(response => {
            console.log(response);
            this.modelResultado = response;

            if (this.modelResultado.estatus == true) {
                this.modelOrdenCompra = this.modelResultado.objeto;
            } else {
                console.log(this.modelResultado.mensaje);
                Swal.fire({
                    title: 'Error',
                    text: 'Ocurrio un error al consultar el detalle de la orden de compra de la requisicion',
                    icon: 'error',
                    timer: 3000,
                    showConfirmButton: false,
                });
            }

            this.isLoading = false;
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

    cargaOrdenCompraDetalle(): void {
        this.isLoading = true;
        this.http.get<Resultado>(`${this.url}api/ordenCompra/getordencompradetalle/${this.idOrdenCompra}`).subscribe(response => {
            console.log(response);
            this.modelResultado = response;

            if (this.modelResultado.estatus == true) {
                this.modelOrdenCompraDetalle = this.modelResultado.objeto;
            } else {
                console.log(this.modelResultado.mensaje);
                Swal.fire({
                    title: 'Error',
                    text: 'Ocurrio un error al consultar el detalle de la orden de compra de la requisicion',
                    icon: 'error',
                    timer: 3000,
                    showConfirmButton: false,
                });
            }

            this.isLoading = false;
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

    

    limpiaModelo() {
        this.modelResultado = {
            estatus: false,
            mensaje: "",
            mensajeError: "",
            objeto: "",
            objetos: []
        };

        this.modelOrdenCompra = {
            idOrden: 0,
            tipo: "",
            estatus: "",
            fechaAlta: "",
            empresa: "",
            proveedor: "",
            idCliente: 0,
            cliente: "",
            elabora: "",
            observacion: "",
            inventario: 0,
            facturado: 0,
            idCredito: 0,
            credito: "",
            diasCredito: 0,
            idAlmacen: 0,
            loading: false,
            iva: 0,
            subTotal: 0,
            total: 0
        }

        this.modelOrdenCompraDetalle = {
            idOrden: 0,
            productos: []
        }

        this.modelResultado = {
            estatus: false,
            mensaje: "",
            mensajeError: "",
            objeto: "",
            objetos: []
        }
    }

    regresaSup() {

    }

    open(idOrdenCompra: number): void {
        let docModal = document.getElementById('modalRequisicionOrdenCompraDetalle');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        this.idOrdenCompra = idOrdenCompra;
        this.cargaOrdenCompra();
        this.cargaOrdenCompraDetalle();
        myModal.show();
    }

    openModalFacutura(idOrdenCompra: number, ingresaInventario: number) {
        this.ordenCompraFacutura.open(idOrdenCompra, ingresaInventario, this.modelOrdenCompra, this.modelOrdenCompraDetalle);
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
}