import { OrdenCompraProducto } from '../models/ordenCompraProducto';

export interface OrdenCompraDetalle {
    idOrden: number;
    productos: OrdenCompraProducto[];
}