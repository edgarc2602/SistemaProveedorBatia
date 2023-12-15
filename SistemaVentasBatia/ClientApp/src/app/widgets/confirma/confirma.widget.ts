import { Component, Input, OnChanges, Output, EventEmitter, SimpleChanges } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'confirma-widget',
    templateUrl: './confirma.widget.html'
})
export class ConfirmaWidget implements OnChanges {
    @Input() mensaje: string = '';
    @Input() titulo: string = '';
    @Output('ansEvent') sendEvent = new EventEmitter<boolean>();

    constructor() {}

    open() { 
        let docModal = document.getElementById('modalConfirma');
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
        let docModal = document.getElementById('modalConfirma');
        let myModal = bootstrap.Modal.getOrCreateInstance(docModal);
        myModal.hide();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}