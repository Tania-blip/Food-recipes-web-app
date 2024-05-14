import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipesTableComponent } from './recipes-table/recipes-table.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule} from '@angular/forms';

import { HttpClientModule } from '@angular/common/http';
@NgModule({
  declarations: [RecipesTableComponent],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    HttpClientModule
  ],
  bootstrap: [RecipesTableComponent]

})
export class AppModule { }
