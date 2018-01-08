//import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core"; // similar Decorator

import { Http, Response, Headers } from "@angular/http";
import { Observable } from "rxjs";
import { Product } from "./product";
import { Order, OrderItem } from "./order";
// some object that represents something like a namespace
// import * as orders from "./order"; 

import 'rxjs/add/operator/map'; // map 

@Injectable()
export class DataService {

    constructor(private http: Http) { } // HttpClient

    private token: string = "";
    private tokenExpiration: Date;

    public products: Product[] = [];
    public order: Order = new Order();

    // Use HTTP and Response
    public loadProducts(): Observable<Product[]> {
        return this.http.get("/api/products")
            .map((result: Response) => this.products = result.json());
    }

    public get loginRequired(): boolean {
        return this.token.length == 0 || this.tokenExpiration > new Date();
    }

    public login(creds) {
        return this.http.post("/account/createtoken", creds)
            .map(response => {
                let tokenInfo = response.json();
                this.token = tokenInfo.token;
                this.tokenExpiration = tokenInfo.expiration;
                return true;
            });
    }

    public checkout() {

        if (!this.order.orderNumber) {
            this.order.orderNumber = this.order.orderDate.getFullYear().toString() + this.order.orderDate.getTime().toString();
        }

        return this.http.post("/api/orders", this.order, {
            headers: new Headers({ "Authorization": "Bearer " + this.token })
        })
            .map(response => {
                this.order = new Order();
                return true;
            });
    }

    public AddToOrder(product: Product) {

        // check item exist
        let item: OrderItem = this.order.items.find(x => x.productId == product.id);
        if (item) {
            item.quantity++;
        }
        else {
            item = new OrderItem();
            item.productId = product.id;
            item.productArtist = product.artist;
            item.productCategory = product.category;
            item.productTitle = product.title;
            item.productSize = product.size;
            item.productPrice = product.price;
            item.quantity = 1;
            this.order.items.push(item);
        }
    }























    ////////////////////////// Use HttpClient 
    //loadProducts() {
    //    return this.http.get("/api/products")
    //        .map((data: any[]) => {
    //            this.products = data;
    //            return true;
    //        });
    //}

    //public products = [{
    //    title: "First Product",
    //    price: 19.99
    //}, {
    //    title: "Second Product",
    //    price: 9.99
    //}, {
    //    title: "Third Product",
    //    price: 8.99
    //}];
}