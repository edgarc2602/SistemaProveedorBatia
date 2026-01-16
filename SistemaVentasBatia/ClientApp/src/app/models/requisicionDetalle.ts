import { RequisicionProducto } from '../models/requisicionProducto';

export interface RequisicionDetalle {
    idRequisicion: number,
    productos: RequisicionProducto[];
} 