import { Component, OnChanges, Output, EventEmitter, SimpleChanges, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetalleMaterial } from '../../models/detallematerial';
declare var bootstrap: any;

@Component({
    selector: 'detallematerialeslistado-widget',
    templateUrl: './detallematerialeslistado.widget.html'
})
export class DetalleMaterialesListadoWidget {
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();
    model: DetalleMaterial = {} as DetalleMaterial;
    sucursal: string;
    tipo: string;
    idListado: number;
    prefijo: string = '';
    constructor(@Inject('BASE_URL') private url: string, private http: HttpClient) { }

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
        this.http.get<DetalleMaterial>(`${this.url}api/entrega/obtenermaterialeslistado/${idListado}`).subscribe(response => {
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
        this.quitarFocoDeElementos();
        this.http.get(`${this.url}api/report/DescargarReporteListadoMaterial/${idListado}`, { responseType: 'arraybuffer' })
            .subscribe(
                (data: ArrayBuffer) => {
                    const pdfDataUrl = this.arrayBufferToDataUrl(data);
                    window.open(pdfDataUrl, '_blank');
                },
                error => {
                    console.error('Error al obtener el archivo PDF', error);
                }
            );
    }

    arrayBufferToDataUrl(buffer: ArrayBuffer): string {
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const dataUrl = URL.createObjectURL(blob);
        return dataUrl;
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }
}