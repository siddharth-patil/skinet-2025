import { catchError, map, of, tap } from 'rxjs';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Address, User } from '../../shared/models/user';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private signalrService = inject(SignalrService);
  currentUser = signal<User | null>(null);

  login(values:any){
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.http.post<User>(this.baseUrl + 'login', values, {params}).pipe(
      tap(()=>this.signalrService.createHubConnection())
    );
  }

  // login(values:any){
  // let params = new HttpParams().append('useCookies', true);

  // return this.http.post<User>(this.baseUrl + 'login', values, {params}).pipe(
  //   tap((user: User) => {
  //     this.currentUser.set(user);   // 🔥 immediately update UI

  //     this.signalrService.createHubConnection();
  //   })
  // );
  // }


  register(values: any){
    return this.http.post(this.baseUrl + 'account/register', values);
  }

  getUserInfo(){
    // return this.http.get<User>(this.baseUrl + 'account/user-info', {withCredentials:true}).subscribe({
    //   next: user => this.currentUser.set(user)
    // })

    return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
      map(user => {
        this.currentUser.set(user);
        return user;
      })//,
      // catchError(() => of(null))
    );
  }

  logout(){
    return this.http.post(this.baseUrl + 'account/logout', {}).pipe(
      tap(()=>this.signalrService.stopHubConnection())
    );
  }

  // logout(){
  // return this.http.post(this.baseUrl + 'account/logout', {}).pipe(
  //   tap(() => {
  //     this.currentUser.set(null);    // Clear UI instantly
  //     this.signalrService.stopHubConnection();
  //   })
  // );
  // }


  updateAddress(address: Address){
    return this.http.post(this.baseUrl + 'account/address', address).pipe(
      tap(()=>{
        this.currentUser.update(user=>{
          if(user) user.address = address;
          return user;
        })
      })
    );
  }

  getAuthState(){
    return this.http.get<{isAuthenticated: boolean}>(this.baseUrl + 'account/auth-status');
  }
}
