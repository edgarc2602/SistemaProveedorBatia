import { Component, OnInit } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'lat-menu',
    templateUrl: './latmenu.component.html'
})
export class LatMenuComponent implements OnInit {

    constructor() {
    }

    ngOnInit(): void {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });
    }
    const enlaces = document.querySelectorAll('.nav-link');

enlaces.forEach(enlace => {
        enlace.addEventListener('click', () => {
            enlace.classList.add('hundimiento');

            // Elimina la clase de hundimiento después de un cierto tiempo (500ms en este caso)
            setTimeout(() => {
                enlace.classList.remove('hundimiento');
            }, 500);
        });
});
}