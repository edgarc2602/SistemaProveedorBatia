import { Component, Inject, ViewChild, ElementRef, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListadoRequisiciones } from '../../models/listadorequisiciones';
import { Requisicion } from '../../models/requisicion';
import Swal from 'sweetalert2';
import { RequisicionDetalleWidget } from '../../widgets/requisiciondetalle/requisiciondetalle.widget';
import { RequisicionOrdenCompraDetalleWidget } from '../../widgets/requisicionordencompradetalle/requisicionordencompradetalle.widget';
declare var bootstrap: any;

@Component({
    selector: 'requisiciones-comp',
    templateUrl: './requisiciones.component.html',
    animations: [fadeInOut]
})
export class RequisicionesComponent implements OnInit {

    @ViewChild(RequisicionDetalleWidget, { static: false }) RequisicionDetalle: RequisicionDetalleWidget;
    @ViewChild(RequisicionOrdenCompraDetalleWidget, { static: false }) RequisicionOrdenCompraDetalle: RequisicionOrdenCompraDetalleWidget;


    model: ListadoRequisiciones = {
        pagina: 0,
        numPaginas: 0,
        rows: 0,
        requisiciones: []
    };
    fltEstatus: number = 0;
    fltMes: number = 0;
    fltAnio: number = 0;


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
    }
    goBack() {
        window.history.back();
    }
    ngOnInit() {
        const fechaActual = new Date();
        this.fltAnio = fechaActual.getFullYear();
        this.cargaRequisiciones(1);
    }

    cargaRequisiciones(pagina: number): void {
        this.http.get<ListadoRequisiciones>(`${this.url}api/requisicion/getrequisiciones/${this.user.idProveedor}/${pagina}/${this.fltMes}/${this.fltEstatus}/${this.fltAnio}`).subscribe(response => {
            this.model = response;
            this.model.pagina = pagina;
            console.log(this.model);
        }, err => {
            Swal.fire({
                title: 'Error',
                text: 'Ocurrio un error al consultar las requisiciones',
                icon: 'error',
                timer: 3000,
                showConfirmButton: false,
            });
        });
    }

    muevePagina(event) {
        this.model.pagina = event;
        this.cargaRequisiciones(this.model.pagina);
    }

    openRequisicionDetalle(requisicion: Requisicion) {
        this.RequisicionDetalle.open(requisicion);
    }

    openOrdenCompraDetalle(idOrdenCompra: number) {
        if (idOrdenCompra > 0) {
            this.RequisicionOrdenCompraDetalle.open(idOrdenCompra);
        } else {
            Swal.fire({
                title: 'Advertencia',
                text: 'Esta requisicion aun no tiene una orden de compra',
                icon: 'warning',
                showConfirmButton: false,
            });
        }
    }

    regresaSup() {

    }

}