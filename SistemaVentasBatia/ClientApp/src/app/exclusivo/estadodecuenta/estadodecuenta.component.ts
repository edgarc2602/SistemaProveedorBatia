import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';


import { StoreUser } from 'src/app/stores/StoreUser';
import { UsuarioRegistro } from 'src/app/models/usuarioregistro';
import { EstadoDeCuenta } from '../../models/estadodecuenta';
import { ListadoEstadoDeCuenta } from '../../models/listadoestadodecuenta';

@Component({
    selector: 'estadodecuenta-comp',
    templateUrl: './estadodecuenta.component.html',
    animations: [fadeInOut],
})
export class EstadoDeCuentaComponent {
    /*@ViewChild(UsuarioAddWidget, { static: false }) addUsu: UsuarioAddWidget;*/
    usuario: UsuarioRegistro = {
        idAutorizacionVentas: 0, idPersonal: 0, autoriza: 0, nombres: '', apellidos: '', puesto: '', telefono: '', telefonoExtension: '', telefonoMovil: '', email: '',
        firma: '', revisa: 0
    }
    model: ListadoEstadoDeCuenta = {
        estadosDeCuenta: [], numPaginas: 0, pagina: 1, rows: 0
    }


    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
    }
    goBack() {
        window.history.back();
    }
    ngOnInit() {
        this.http.get<ListadoEstadoDeCuenta>(`${this.url}api/cuenta/getestadodecuenta/${this.user.idProveedor}/${this.model.pagina}`).subscribe(response => {
            this.model = response;
        })
    }

    muevePagina(event) {
        this.model.pagina = event;
        this.ngOnInit();
    }
}