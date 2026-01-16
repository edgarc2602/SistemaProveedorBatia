import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'presentacion-widget',
    templateUrl: './presentacion.widget.html'
})
export class PresentacionWidget implements OnChanges {
    @Input() mensaje: string = '';
    @Input() titulo: string = '';
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();
    videoUrl: string = 'https://www.singa.com.mx/Doctos/proveedores/proveedoresgrubit.mp4';
    constructor() {}

    open() { 
        let docModal = document.getElementById('modalPresentacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.show();
    }

    acepta() {
        this.sendEvent.emit(true);
        this.close();
    }

    cancela() {
        this.sendEvent.emit(false);
        this.close();
    }

    close() {
        let docModal = document.getElementById('modalPresentacion');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}