import { EstadoDeCuenta } from './estadodecuenta';
export interface ListadoEstadoDeCuenta {
    estadosDeCuenta: EstadoDeCuenta[];
    pagina: number;
    rows: number;
    numPaginas: number;
}