import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
    model: ListadoOrdenCompra = {
        ordenes: [], numPaginas: 0, pagina: 1, rows: 0
    }
    idProveedor: number = 1186;
    fechaInicio: string = '';
    fechaFin: string = '';

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) { }

    ngOnInit() {
        this.getDias();
        this.obtenerOrdenes();
    }   
    obtenerOrdenes() {
        this.idProveedor = this.user.idProveedor;
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
        this.quitarFocoDeElementos();
    }

    openCargarFacturas(idOrden: number, empresa: string, cliente: string, total: number) {
        this.quitarFocoDeElementos();
        this.upfact.open(idOrden, empresa, cliente,total);
    }
    returnModal($event) {
        this.obtenerOrdenes();
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}

