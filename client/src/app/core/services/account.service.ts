import { catchError, map, of, tap, throwError } from 'rxjs';
import { computed, inject, Injectable, signal } from '@angular/core';
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
  isAdmin = computed(()=>{
    const roles = this.currentUser()?.roles;
    return Array.isArray(roles) ? roles.includes('Admin'):roles ==='Admin';
  })

  login(values:any){
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.http.post<User>(this.baseUrl + 'login', values, {params}).pipe(
      tap(()=>this.signalrService.createHubConnection())
    );
  }


  register(values: any){
    return this.http.post(this.baseUrl + 'account/register', values);
  }

  // getUserInfo(){

  //   return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
  //     map(user => {
  //       this.currentUser.set(user);
  //       return user;
  //     })//,
  //     // catchError(() => of(null))
  //   );
  // }
  getUserInfo() {
  return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
    tap(user => this.currentUser.set(user)),
    catchError(err => {
      // 401 = not logged in → just return null instead of breaking the app
      if (err.status === 401) {
        this.currentUser.set(null);
        return of(null);
      }
      // other errors should still bubble up if you want
      return throwError(() => err);
    })
  );
  }

  logout(){
    return this.http.post(this.baseUrl + 'account/logout', {}).pipe(
      tap(()=>this.signalrService.stopHubConnection())
    );
  }


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
