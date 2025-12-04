import { ShopService } from './core/services/shop.service';
import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';
import { ShopComponent } from './features/shop/shop.component';
import { SignalrService } from './core/services/signalr.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'skinet';
}
