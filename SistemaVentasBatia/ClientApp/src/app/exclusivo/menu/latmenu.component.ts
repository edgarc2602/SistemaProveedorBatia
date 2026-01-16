import { Component, OnInit, ViewChild } from '@angular/core';
import { PresentacionWidget } from '../../widgets/presentacion/presentacion.widget';
declare var bootstrap: any;

@Component({
    selector: 'lat-menu',
    templateUrl: './latmenu.component.html'
})
export class LatMenuComponent implements OnInit {
    isDarkTheme: boolean = false;

    @ViewChild(PresentacionWidget, { static: false }) presentacionWidget: PresentacionWidget;

    constructor() {
    }

    ngOnInit(): void {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });
    }
    toggleTheme() {
        this.isDarkTheme = !this.isDarkTheme;
        if (this.isDarkTheme) {
            document.body.classList.add('dark-theme');
        } else {
            document.body.classList.remove('dark-theme');
        }
        this.quitarFocoDeElementos();
    }

    quitarFocoDeElementos(): void {
        const elementos = document.querySelectorAll('button, input[type="text"]');
        elementos.forEach((elemento: HTMLElement) => {
            elemento.blur();
        });
    }

    openPresentacion() {
        this.presentacionWidget.open();
    }
}