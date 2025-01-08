import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePage } from './home.page';
import { ContactPage } from '../pages/contact/contact.page';
import { ContactPageModule } from '../pages/contact/contact.module';

const routes: Routes = [
  {
    path: '',
    component: HomePage,
    children: [
      {
        path: 'contact-us',
        component: ContactPageModule,
        //loadChildren: () => import('.././pages/contact/contact.module').then( m => m.ContactPageModule)
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomePageRoutingModule {}
