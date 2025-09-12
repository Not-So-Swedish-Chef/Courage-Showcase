import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { SignupComponent } from './pages/signup/signup.component';

import { EventFormComponent } from './pages/events/event-form/event-form.component';
import { EventListComponent } from './components/event-list/event-list.component';
import { EventDetailComponent } from './components/event-detail/event-detail.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  // { path: 'events', component: EventsComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'events/create', component: EventFormComponent },
  { path: 'events', component: EventListComponent },
  { path: 'events/:id', component: EventDetailComponent },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
