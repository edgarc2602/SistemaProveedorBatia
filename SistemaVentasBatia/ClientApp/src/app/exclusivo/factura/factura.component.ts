import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListadoOrdenCompra } from 'src/app/models/listadoordencompra';
import { CargarFacturaWidget } from 'src/app/widgets/cargarfactura/cargarfactura.widget';
@Component({
    selector: 'factura-comp',
    templateUrl: './factura.component.html',
    animations: [fadeInOut],
})
export class FacturaComponent {
    @ViewChild(CargarFacturaWidget, { static: false }) upfact: CargarFacturaWidget;
    /*@ViewChild(UsuarioAddWidget, { static: false }) addUsu: UsuarioAddWidget;*/
    model: ListadoOrdenCompra = {
        ordenes: [], numPaginas: 0, pagina: 1, rows: 0
    }
    idProveedor: number = 1108;
    fechaInicio: string = '';
    fechaFin: string = '';

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        //this.http.get<ListadoOrdenCompra>(`${this.url}api/factura/obtenerordenescompra/${this.idProveedor}/${this.fechaInicio}/${this.fechaFin}/${this.model.pagina}`).subscribe(response => {
        //    this.model = response;
        //})
    }
    ngOnInit() {
        this.getDias();
        this.obtenerOrdenes();
    }   
    obtenerOrdenes() {
        this.http.get<ListadoOrdenCompra>(`${this.url}api/factura/ObtenerOrdenesCompra/${this.idProveedor}/${this.model.pagina}/${this.fechaInicio}/${this.fechaFin}`).subscribe(response => {
            this.model = response;
        })
    }
    goBack() {
        window.history.back();
    }
    muevePagina(event) {
        this.model.pagina = event;
        this.obtenerOrdenes();
    }

    obtenerPrimerDiaDelMes(): string {
        const hoy = new Date();
        const primerDiaDelMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
        return primerDiaDelMes.toISOString().slice(0, 10);
    }

    obtenerDiaActual(): string {
        const diaActual = new Date();
        return diaActual.toISOString().slice(0, 10);
    }

    getDias() {
        this.fechaInicio = this.obtenerPrimerDiaDelMes();
        this.fechaFin = this.obtenerDiaActual();
    }
    imprimirOrden(idOrden: number) {

    }

    openCargarFacturas(idOrden: number, empresa: string, cliente: string) {
        this.upfact.open(idOrden, empresa, cliente);
    }

}

