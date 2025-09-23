import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { SignupComponent } from './pages/signup/signup.component';

import { EventFormComponent } from './pages/events/event-form/event-form.component';
import { EventListComponent } from './components/event-list/event-list.component';
import { EventDetailComponent } from './components/event-detail/event-detail.component';
import { ProfileEditComponent } from './pages/profile-edit/profile-edit.component';
import { MyEventsComponent } from './pages/my-events/my-events.component';
import { HostProfileComponent } from './pages/host-profile/host-profile.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  // { path: 'events', component: EventsComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'host/:id', component: HostProfileComponent },
  { path: 'profile/edit', component: ProfileEditComponent },
  { path: 'events/create', component: EventFormComponent },
  { path: 'events/update/:id', component: EventFormComponent },
  { path: 'events', component: EventListComponent },
  { path: 'my-events', component: MyEventsComponent },
  { path: 'events/:id', component: EventDetailComponent },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
