import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from "@angular/common/http";
import { HttpModule } from "@angular/http";
import { RouterModule } from "@angular/router";
import { FormsModule } from "@angular/forms";

import { AppComponent } from './app.component';
import { ProductList } from "./shop/productList.component";
import { DataService } from "../app/shared/dataService";
import { Cart } from "./shop/cart.component";
import { Shop } from "./shop/shop.component";
import { Checkout } from "./checkout/checkout.component";
import { Login } from "./login/login.component";


let routes = [
    { path: "", component: Shop },
    { path: "checkout", component: Checkout },
    { path: "login", component: Login }
];


@NgModule({
  declarations: [
      AppComponent,
      ProductList,
      Cart,
      Shop,
      Checkout,
      Login
  ],
  imports: [
      BrowserModule,
      HttpClientModule,
      HttpModule,
      FormsModule,
      RouterModule.forRoot(routes, {
          useHash: true,
          enableTracing: false // for Debuging of the Routes
      })
  ],
  providers: [
      DataService // can be add into the dependency injection system
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
