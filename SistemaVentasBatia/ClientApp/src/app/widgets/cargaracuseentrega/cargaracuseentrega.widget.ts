import { Component, OnChanges, Output, EventEmitter, SimpleChanges, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetalleMaterial } from '../../models/detallematerial';

declare var bootstrap: any;

@Component({
    selector: 'cargaracuseentrega-widget',
    templateUrl: './cargaracuseentrega.widget.html'
})
export class CargarAcuseEntregaWidget{
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();
    model: DetalleMaterial = {} as DetalleMaterial;
    sucursal: string;
    tipo: string;
    idListado: number;
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {}

    open(idListado: number, sucursal: string, tipo: string) {
        this.idListado = idListado;
        this.sucursal = sucursal;
        this.tipo = tipo;
        let docModal = document.getElementById('modalCargarAcuseEntrega');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }
    //obtenerMaterialesListado(idListado: number) {
    //    this.http.get <DetalleMaterial>(`${this.url}api/entrega/obtenermaterialeslistado/${idListado}`).subscribe(response => {
    //        this.model = response;
    //    })
    //}

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalCargarAcuseEntrega');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    concluirEntrega() {

    }
}