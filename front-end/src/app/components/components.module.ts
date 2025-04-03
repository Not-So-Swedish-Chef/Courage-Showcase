import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './page-elements/header/header.component';
import { FooterComponent } from './page-elements/footer/footer.component';
import { IonicModule } from '@ionic/angular';
import { FormsModule } from '@angular/forms';
import { EventCardComponent } from './events/event-card/event-card.component';



@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    EventCardComponent
  ],
  imports: [
    CommonModule,
    IonicModule,
    FormsModule
  ],
  exports: [
    HeaderComponent,
    FooterComponent,
    EventCardComponent
  ]
})
export class ComponentsModule { }
