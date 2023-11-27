import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { fadeInOut } from 'src/app/fade-in-out';


import { StoreUser } from 'src/app/stores/StoreUser';
import { UsuarioRegistro } from 'src/app/models/usuarioregistro';

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

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {

        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));
    }
    goBack() {
        window.history.back();
    }
}

