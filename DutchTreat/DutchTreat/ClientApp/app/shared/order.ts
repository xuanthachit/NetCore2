﻿import * as _ from "lodash";


export class Order {
    orderId: number;
    orderDate: Date = new Date();
    orderNumber: string;
    items: Array<OrderItem> = new Array<OrderItem>();

    get subtotal(): number {
        return _.sum(_.map(this.items, i => i.productPrice * i.quantity));
    };

    //set subtotal(value: number) {

    //}
}

export class OrderItem {
    id: number;
    quantity: number;
    productPrice: number;
    productId: number;
    productCategory: string;
    productSize: string;
    productTitle: string;
    productArtist: string;
}

