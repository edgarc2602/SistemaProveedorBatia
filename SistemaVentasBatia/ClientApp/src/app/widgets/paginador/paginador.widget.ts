import { Component, OnChanges, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'pagina-widget',
    templateUrl: './paginador.widget.html'
})
export class PaginaWidget implements OnChanges {
    @Input() pagina: number = 0;
    @Input() numPaginas: number = 0;
    @Input() rows: number = 0;
    @Output('chgEvent') changeEvent = new EventEmitter<number>();
    bloqueActual: number = 1;

    constructor() {}

    makePages(): number[] {
        const paginas = [];
        const inicioBloque = (this.bloqueActual - 1) * 10 + 1;
        const finBloque = Math.min(this.bloqueActual * 10, this.numPaginas);

        for (let i = inicioBloque; i <= finBloque; i++) {
            paginas.push(i);
        }

        return paginas;
    }

    toPrev() {
        this.move(this.pagina - 1);
    }

    toNext() {
        this.move(this.pagina + 1);
    }

    move(p: number) {
        this.changeEvent.emit(p);
    }

    ngOnChanges(changes: SimpleChanges): void {
    }


    toPrevBlock() {
        if (this.bloqueActual > 1) {
            this.bloqueActual--;
        }
    }

    toNextBlock() {
        const ultimaPaginaBloque = this.bloqueActual * 10;
        if (ultimaPaginaBloque <= this.numPaginas) {
            this.bloqueActual++;
        }
    }
}