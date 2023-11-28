import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListadoMateriales } from 'src/app/models/listadomateriales';
import { Catalogo } from '../../models/catalogo';

@Component({
    selector: 'entrega-comp',
    templateUrl: './entrega.component.html',
    animations: [fadeInOut],
})
export class EntregaComponent {
    /*@ViewChild(UsuarioAddWidget, { static: false }) addUsu: UsuarioAddWidget;*/
    model: ListadoMateriales = {
        listas: [], numPaginas: 0, pagina: 1, rows: 0
    }
    meses: Catalogo[];
    tipoListado : Catalogo[]
    mes: number = 0;
    anio: number = 0;
    idEstado: number = 0;
    tipo: number = 0;
    idProveedor: number = 35;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        http.get<Catalogo[]>(`${url}api/catalogo/obtenermeses`).subscribe(response => {
            this.meses = response;
        })
        http.get<Catalogo[]>(`${url}api/catalogo/obtenertipolistado`).subscribe(response => {
            this.tipoListado = response;
        })
    }
    ngOnInit() {
        const fechaActual = new Date();
        this.anio = fechaActual.getFullYear();
        const fechaActualMes = new Date();
        this.mes = fechaActualMes.getMonth() + 1;
        this.obtenerListados();
    }
    obtenerListados() {
        //this.user.idPersonal
        this.http.get<ListadoMateriales>(`${this.url}api/entrega/obtenerlistados/${this.mes}/${this.anio}/${this.idProveedor}/${this.idEstado}/${this.tipo}/${this.model.pagina}`).subscribe(response => {
            this.model = response;
        }, err => console.log(err));
    }
    goBack() {
        window.history.back();
    }
    muevePagina(event) {
        this.model.pagina = event;
        this.obtenerListados();
    }

    obtenerListado(idListado: number) {

    }
}
