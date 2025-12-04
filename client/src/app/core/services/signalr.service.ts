// import { HubConnectionBuilder } from './../../../../node_modules/@microsoft/signalr/src/HubConnectionBuilder';
// import { HubConnection, HubConnectionState } from './../../../../node_modules/@microsoft/signalr/src/HubConnection';
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  hubUrl = environment.hubUrl;
  hubConnection?: HubConnection;
  orderSignal = signal<Order | null>(null);

  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl,{
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build();

    // this.hubConnection?.start().catch(error => console.log('Error establishing connection: ', error));

    // this.hubConnection.on('OrderCompleteNotification',(order:Order)=>{
    //   this.orderSignal.set(order);
    this.hubConnection
      ?.start()
      .then(() => console.log("SignalR hub connected"))
      .catch(err => console.log("Error connecting hub", err));

    this.hubConnection.on("OrderCompleteNotification", (order: Order) => {
      console.log("Order received from SignalR:", order);
      this.orderSignal.set(order);
    })
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection?.stop().catch(error => console.log('Error stopping connection: ', error));
    }
  }
}
