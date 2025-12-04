// import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
// import { inject } from '@angular/core';
// import { NavigationExtras, Router } from '@angular/router';
// import { catchError, throwError } from 'rxjs';
// import { SnackbarService } from '../services/snackbar.service';

// export const errorInterceptor: HttpInterceptorFn = (req, next) => {
//   const router = inject(Router);
//   const snackbar = inject(SnackbarService);

//   return next(req).pipe(
//     catchError((err: HttpErrorResponse) => {
//       if (err.status === 400 || err.status === 401) {
//         if (err.error.errors) {
//           const modelStateErrors = [];
//           for (const key in err.error.errors) {
//             if (err.error.errors[key]) {
//               modelStateErrors.push(err.error.errors[key]);
//             }
//           }
//           throw modelStateErrors.flat();
//         } else {
//           snackbar.error(err.error.title || err.error);
//         }
//       }
//       if (err.status === 404) {
//         router.navigateByUrl('/not-found');
//       }
//       if (err.status === 500) {
//         const navigationExtras:NavigationExtras = {state: {error:err.error}}
//         router.navigateByUrl('/server-error', navigationExtras);
//       }
//       return throwError(() => err);
//     })
//   );
// };
//------------------------------------------------------------------------------//
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, of, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

// export const errorInterceptor: HttpInterceptorFn = (req, next) => {
//   const router = inject(Router);
//   const snackbar = inject(SnackbarService);

//   return next(req).pipe(
//     catchError((err: HttpErrorResponse) => {

//       // Handle 400 & 401 gracefully
//       if (err.status === 400 || err.status === 401) {

//         // If Model state validation errors exist
//         if (err.error && err.error.errors) {
//           const modelStateErrors: string[] = [];
//           for (const key in err.error.errors) {
//             if (err.error.errors[key]) {
//               modelStateErrors.push(err.error.errors[key]);
//             }
//           }
//           return throwError(() => modelStateErrors.flat());
//         }

//         // Otherwise show message safely
//         snackbar.error(err.error?.title || err.error || "Unauthorized access");
//         return throwError(() => err);
//       }

//       if (err.status === 404) {
//         router.navigateByUrl('/not-found');
//       }

//       if (err.status === 500) {
//         const navigationExtras: NavigationExtras = { state: { error: err.error } };
//         router.navigateByUrl('/server-error', navigationExtras);
//       }

//       snackbar.error("Something went wrong");
//       return throwError(() => err);
//     })
//   );
// };


export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {

      // Ignore initial user check 401 to prevent app crash
      if (err.status === 401 && req.url.includes('account/user-info')) {
        return of(null);
      }

      if (err.status === 400 || err.status === 401) {
        if (err.error?.errors) {
          const modelStateErrors = [];
          for (const key in err.error.errors) {
            if (err.error.errors[key]) {
              modelStateErrors.push(err.error.errors[key]);
            }
          }
          snackbar.error(modelStateErrors.join(', '));
        } else {
          snackbar.error(err.error?.title || 'Unauthorized request');
        }

        return of(null);   // 👈 STOP THROWING ERROR HERE
      }

      if (err.status === 404) {
        router.navigateByUrl('/not-found');
        return of(null);
      }

      if (err.status === 500) {
        const navigationExtras: NavigationExtras = { state: { error: err.error }};
        router.navigateByUrl('/server-error', navigationExtras);
        return of(null);
      }

      snackbar.error('Something went wrong');
      return of(null);   // 👈 Always return safe value
    })
  );
};

