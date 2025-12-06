import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { catchError, forkJoin, of, startWith, tap } from 'rxjs';
import { AccountService } from './account.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private signalrService = inject(SignalrService);

  // init() {
  //   const cartId = localStorage.getItem('cart_id');
  //   const cart$ = cartId ? this.cartService.getCart(cartId) : of(null);

  //   // return cart$;
  //   return forkJoin({
  //     cart: cart$,
  //     user: this.accountService.getUserInfo().pipe(
  //       tap(user=>{
  //         if(user) this.signalrService.createHubConnection();
  //       })
  //     )
  //   })
  // }

  init() {
    const cartId = localStorage.getItem('cart_id');
    const cart$ = cartId
      ? this.cartService.getCart(cartId).pipe(
          catchError(() => of(null)) // cart load errors won’t break startup
        )
      : of(null);

    const user$ = this.accountService.getUserInfo().pipe(
      startWith(null), 
      tap((user) => {
        if (user) {
          this.signalrService.createHubConnection();
        }
      }),
      catchError(() => of(null)) // VERY important
    );

    return forkJoin({
      cart: cart$,
      user: user$,
    });
  }
}
