import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListadoOrdenCompra } from 'src/app/models/listadoordencompra';
import { CargarFacturaWidget } from 'src/app/widgets/cargarfactura/cargarfactura.widget';
import { Catalogo } from '../../models/catalogo';
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
    statusc: Catalogo[];
    idStatus: number = 1;
    isLoading: boolean = false;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/factura/GetStatusOrdenCompra`).subscribe(response => {
            this.statusc = response;
        })
    }

    ngOnInit() {
        this.getDias();
        this.obtenerOrdenes(1);
    }
    obtenerOrdenes(filtro: number) {
        if (filtro == 1) {
            this.model.ordenes = [];
            this.model.pagina = 1;
            this.isLoading = true;
        }
        this.idProveedor = this.user.idProveedor;
        this.http.get<ListadoOrdenCompra>(`${this.url}api/factura/ObtenerOrdenesCompra/${this.idProveedor}/${this.model.pagina}/${this.fechaInicio}/${this.fechaFin}/${this.idStatus}`).subscribe(response => {
            setTimeout(() => {
                this.model = response;
                this.isLoading = false;
            }, 300);
        }, err => {
            setTimeout(() => {
                this.isLoading = false;
            }, 300);
        });
    }
    goBack() {
        window.history.back();
    }
    muevePagina(event) {
        this.model.pagina = event;
        this.obtenerOrdenes(2);
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
    imprimirOrden(idOrden: number, tipo: string) {
        this.quitarFocoDeElementos();
        const orden = this.model.ordenes.find((orden) => orden.idOrden === idOrden && orden.tipo === tipo);
        if (orden) {
            orden.loading = true;

            this.http.get(`${this.url}api/report/DescargarReporteOrdenCompra/${idOrden}/${tipo}`, { responseType: 'arraybuffer' })
                .subscribe(
                    (data: ArrayBuffer) => {
                        const file = new Blob([data], { type: 'application/pdf' });
                        const fileURL = URL.createObjectURL(file);
                        const width = 800;
                        const height = 550;
                        const left = window.innerWidth / 2 - width / 2;
                        const top = window.innerHeight / 2 - height / 2;
                        const newWindow = window.open(fileURL, '_blank', `width=${width}, height=${height}, top=${top}, left=${left}`);
                        if (newWindow) {
                            newWindow.focus();
                        } else {
                            alert('La ventana emergente ha sido bloqueada. Por favor, permite ventanas emergentes para este sitio.');
                        }
                        orden.loading = false;
                    },
                    error => {
                        console.error('Error al obtener el archivo PDF', error);
                        orden.loading = false;
                    }
                );
        }

    }
    arrayBufferToDataUrl(buffer: ArrayBuffer): string {
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const dataUrl = URL.createObjectURL(blob);
        return dataUrl;
    }

    openCargarFacturas(idOrden: number, empresa: string, cliente: string, total: number) {
        this.quitarFocoDeElementos();
        this.upfact.open(idOrden, empresa, cliente, total);
    }
    returnModal($event) {
        this.obtenerOrdenes(2);
    }
    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}

