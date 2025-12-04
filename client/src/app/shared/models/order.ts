export interface Order {
  id: number
  orderDate: string
  shippingPrice: number
  deliveryMethod: string
  orderItems: OrderItem[]
  buyerEmail: string
  paymentSummary: PaymentSummary
  shippingAddress: ShippingAddress
  subtotal: number
  total: number
  status: string
  paymentIntentId: string
}

export interface OrderItem {
  productId: number
  productName: string
  pictureUrl: string
  price: number
  quantity: number
}

export interface PaymentSummary {
  last4: number
  brand: string
  expMonth: number
  expYear: number
}

export interface ShippingAddress {
  name: string
  line1: string
  line2?: string
  city: string
  state: string
  postalCode: string
  country: string
}

export interface OrderToCreate{
    cartId: string;
    deliveryMethodId: number;
    shippingAddress: ShippingAddress;
    paymentSummary: PaymentSummary;
}
