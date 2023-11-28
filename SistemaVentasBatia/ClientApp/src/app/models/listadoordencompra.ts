import { OrdenCompra } from './ordencompra';

export interface ListadoOrdenCompra {
    ordenes: OrdenCompra[];
    pagina: number;
    rows: number;
    numPaginas: number;
}