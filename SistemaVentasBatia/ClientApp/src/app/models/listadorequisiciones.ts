import { Requisicion } from '../models/requisicion';

export interface ListadoRequisiciones {
    pagina: number;
    numPaginas: number;
    rows: number;
    requisiciones: Requisicion[];
} 