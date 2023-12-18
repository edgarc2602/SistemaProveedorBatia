import { Component, OnChanges, Output, EventEmitter, SimpleChanges, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetalleMaterial } from '../../models/detallematerial';

declare var bootstrap: any;

@Component({
    selector: 'detallematerialeslistado-widget',
    templateUrl: './detallematerialeslistado.widget.html'
})
export class DetalleMaterialesListadoWidget{
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();
    model: DetalleMaterial = {} as DetalleMaterial;
    sucursal: string;
    tipo: string;
    idListado: number;
    prefijo: string = '';
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) {}

    open(idListado: number, sucursal: string, tipo: string, prefijo: string) {
        this.idListado = idListado;
        this.sucursal = sucursal;
        this.tipo = tipo;
        this.prefijo = prefijo;
        this.obtenerMaterialesListado(idListado);
        let docModal = document.getElementById('modalDetalleMaterialesListado');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }
    obtenerMaterialesListado(idListado: number) {
        this.http.get <DetalleMaterial>(`${this.url}api/entrega/obtenermaterialeslistado/${idListado}`).subscribe(response => {
            this.model = response;
        })
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalDetalleMaterialesListado');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    obtenerReporte(idListado: number, prefijo: string) {
        this.http.get<any>(`${this.url}api/entrega/generarreporte/${idListado}/${prefijo}`, { responseType: 'blob' as 'json' })
            .subscribe(response => {
                const file = new Blob([response], { type: 'application/pdf' }); // Cambia el tipo MIME si el reporte es de otro formato
                const fileURL = URL.createObjectURL(file);
                window.open(fileURL, '_blank');
            });
        this.quitarFocoDeElementos();
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}