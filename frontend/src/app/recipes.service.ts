import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recipe} from './models/recipe.model';
import { ExtendedRecipe } from './models/extendedRecipe.model';

@Injectable({
  providedIn: 'root'
})
export class RecipesService {

  constructor(private http: HttpClient) { }

  private baseUrl = 'http://localhost:5274/Recipes';
  getRecipes(page: number, pageSize: number): Observable<any> {
    return this.http.get(`http://localhost:5274/Recipes/getAll?page=${page}&limit=${pageSize}`);
  }

  getRecipeById(id: string): Observable<ExtendedRecipe> {
    return this.http.get<ExtendedRecipe>(`${this.baseUrl}/get/${id}`);
  }

  getRecipesByAuthor(authorName: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/authorRecipes/${authorName}`);
  }

  getRecipesByName(name: string): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.baseUrl}/search/${name}`);
  }

  getRecipesByIngredients(ingredients: string[]): Observable<Recipe[]> {
    const params = new HttpParams().set('ingredients', ingredients.join(','));
    return this.http.get<Recipe[]>(`${this.baseUrl}/filterByIngredients`, { params });
  }
}
