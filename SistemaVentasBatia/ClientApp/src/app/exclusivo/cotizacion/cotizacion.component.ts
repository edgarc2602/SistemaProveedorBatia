import { Component, Inject, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { ListaCotizacion } from 'src/app/models/listacotizacion';
import { Prospecto } from 'src/app/models/prospecto';
import { ItemN } from 'src/app/models/item';

import { CotizaResumenLim } from 'src/app/models/cotizaresumenlim';
//import { DireccionCotizacion } from '../../models/direccioncotizacion';
import { ListaDireccion } from '../../models/listadireccion';
import { Cotizacion } from '../../models/cotizacion';

import { EliminaWidget } from 'src/app/widgets/elimina/elimina.widget';
import { EditarCotizacion } from 'src/app/widgets/editacotizacion/editacotizacion.widget';

import { StoreUser } from 'src/app/stores/StoreUser';
import { fadeInOut } from 'src/app/fade-in-out';
@Component({
    selector: 'cotizacion',
    templateUrl: './cotizacion.component.html',
    animations: [fadeInOut],
    
})
export class CotizacionComponent implements OnInit, OnDestroy {
    sub: any;
    lcots: ListaCotizacion = {
        idProspecto: 0, idServicio: 0, pagina: 1, numPaginas: 0,
        rows: 0, cotizaciones: [], idEstatusCotizacion: 0, idAlta: '', total: 0
    };
    lsers: ItemN[] = [];
    lests: ItemN[] = [];
    lpros: Prospecto[] = [];

    model: CotizaResumenLim = {
        idCotizacion: 0, idProspecto: 0, salario: 0, cargaSocial: 0, provisiones: 0,
        material: 0, uniforme: 0, equipo: 0, herramienta: 0, servicio: 0,
        subTotal: 0, indirecto: 0, utilidad: 0, total: 0, idCotizacionOriginal: 0, idServicio: 0, nombreComercial: '', indirectoPor: '', utilidadPor: '', csvPor: '', comisionSV: 0, comisionExtPor: '', comisionExt:0
    };

    lsdir: ListaDireccion = {} as ListaDireccion;

    idpro: number = 0;
    @ViewChild(EliminaWidget, { static: false }) eliw: EliminaWidget;
    @ViewChild(EditarCotizacion, { static: false }) ediw: EditarCotizacion;

    estatus: number = 1;
    autorizacion: number = 0;




    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, private route: ActivatedRoute, public user: StoreUser) {
       
        http.get<ItemN[]>(`${url}api/prospecto/getservicio`).subscribe(response => {
            this.lsers = response;
        }, err => console.log(err));
        http.get<ItemN[]>(`${url}api/cotizacion/getestatus`).subscribe(response => {
            this.lests = response;
        }, err => console.log(err));
        http.get<number>(`${url}api/cotizacion/obtenerautorizacion/${user.idPersonal}`).subscribe(response => {
            this.autorizacion = response;
        }, err => console.log(err));




    }

    nuevo() {
        this.lcots = {
            idProspecto: 0, idServicio: 0, pagina: 1, numPaginas: 0,
            rows: 0, cotizaciones: [], idEstatusCotizacion: 0, idAlta: '', total: 0
        };
    }
    init() {
        let fil: string = (this.lcots.idEstatusCotizacion > 0 ? `estatus=1` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idServicio > 0 ? `servicio=${this.lcots.idServicio}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idProspecto > 0 ? `idProspecto=${this.lcots.idProspecto}` : '');
        if (fil.length > 0) fil = '?' + fil;
        this.http.get<ListaCotizacion>(`${this.url}api/cotizacion/${this.user.idPersonal}/${this.lcots.pagina}${fil}`).subscribe(response => {
            this.lcots = response;
        }, err => console.log(err));
        this.http.post<Prospecto[]>(`${this.url}api/prospecto/getcatalogo`, this.user.idPersonal).subscribe(response => {
            this.lpros = response;
        }, err => console.log(err));
    }

    lista() {
        let fil: string = (this.lcots.idEstatusCotizacion > 0 ? `estatus=${this.lcots.idEstatusCotizacion}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idServicio > 0 ? `servicio=${this.lcots.idServicio}` : '');
        if (fil.length > 0) fil += '&';
        fil += (this.lcots.idProspecto > 0 ? `idProspecto=${this.lcots.idProspecto}` : '');
        if (fil.length > 0) fil = '?' + fil;
        this.http.get<ListaCotizacion>(`${this.url}api/cotizacion/${this.user.idPersonal}/${this.lcots.pagina}${fil}`).subscribe(response => {
            this.lcots = response;
        }, err => console.log(err));
    }

    busca() {
        this.lcots.pagina = 1;
        this.lista();
    }

    getDet(id: number, ser: string) {
        console.log(`${id} : ${ser}`);
    }

    muevePagina(event) {
        this.lcots.pagina = event;
        this.lista();
    }

    ngOnInit(): void {
        

        this.sub = this.route.params.subscribe(params => {
            let idp: number = +params['idp'];
            if (idp > 0) {
                this.lcots.idProspecto = idp;
            } else {
                this.nuevo();
            }
            this.lista();
            this.init();
        });
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }

    getDirs() {
        this.http.get<ListaDireccion>(`${this.url}api/cotizacion/limpiezadirectorio/${this.model.idCotizacion}`).subscribe(response => {
            this.lsdir = response;
        }, err => console.log(err));
    }


    elige(idCotizacion) {
        this.idpro = idCotizacion;
        this.eliw.titulo = 'Desactivar'; //error
        this.eliw.mensaje = 'El estatus cambiara a "Inactivo"';
        this.eliw.open();
    }

    elimina($event) {
        if ($event) {
            this.http.post<Cotizacion>(`${this.url}api/cotizacion/EliminarCotizacion`, this.idpro).subscribe(response => {
            }, err => console.log(err));
        }
    }
    editar(idCotizacion: number, prospecto: string, servicio: string) {
        this.ediw.openSel(idCotizacion, prospecto, servicio);
    }
    goBack() {
        window.history.back();
    }
}
