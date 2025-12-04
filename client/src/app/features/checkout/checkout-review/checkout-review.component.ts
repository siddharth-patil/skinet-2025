import { ConfirmationToken } from '@stripe/stripe-js';
import { CurrencyPipe } from '@angular/common';
import { CartService } from './../../../core/services/cart.service';
import { Component, inject, Input, input } from '@angular/core';
import { AddressPipe } from "../../../shared/pipes/address-pipe";
import { PaymentCardPipe } from "../../../shared/pipes/payment-card-pipe";

@Component({
  selector: 'app-checkout-review',
  imports: [
    CurrencyPipe,
    AddressPipe,
    PaymentCardPipe
],
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss'
})
export class CheckoutReviewComponent {
  cartService = inject(CartService);
  @Input() confirmationToken?: ConfirmationToken;
}
