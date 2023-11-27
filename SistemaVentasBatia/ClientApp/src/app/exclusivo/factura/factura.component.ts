import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Catalogo } from 'src/app/models/catalogo';

import { fadeInOut } from 'src/app/fade-in-out';


import { StoreUser } from 'src/app/stores/StoreUser';

@Component({
    selector: 'factura-comp',
    templateUrl: './factura.component.html',
    animations: [fadeInOut],
})
export class FacturaComponent {
    /*@ViewChild(UsuarioAddWidget, { static: false }) addUsu: UsuarioAddWidget;*/

    //pues: Catalogo[] = [];
    //selPuesto: number = 0;

    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient, public user: StoreUser) {
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));
        //http.get<Catalogo[]>(`${url}api/catalogo/getpuesto`).subscribe(response => {
        //    this.pues = response;
        //}, err => console.log(err));

    }
    chgServicio() {

    }
    goBack() {
        window.history.back();
    }

    
}

