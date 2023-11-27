import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';
import { StoreUser } from 'src/app/stores/StoreUser';
import { ListadoMateriales } from 'src/app/models/listadomateriales';

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
    mes: number = 10;
    anio: number = 2023;
    idEstado: number = 0;
    tipo: number = 0;
    idPersonal: number = 123;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));
    }
    obtenerListados() {
        //this.user.idPersonal
        this.http.get<ListadoMateriales>(`${this.url}api/entrega/obtenerlistados/${this.mes}/${this.anio}/${this.idPersonal}/${this.idEstado}/${this.tipo}/${this.model.pagina}`).subscribe(respose => {
            this.model = respose;
        }, err => console.log(err));
    }
    goBack() {
        window.history.back();
    }
    muevePagina(event) {
        this.model.pagina = event;
        this.obtenerListados();
    }
}
