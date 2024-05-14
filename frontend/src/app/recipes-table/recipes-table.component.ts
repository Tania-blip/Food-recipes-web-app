import { Component, OnInit } from '@angular/core';
import { RecipesService } from '../recipes.service';
import { Recipe } from '../models/recipe.model';
import { ExtendedRecipe } from '../models/extendedRecipe.model';

@Component({
  selector: 'app-root',
  templateUrl: './recipes-table.component.html',
  styleUrls: ['./recipes-table.component.css'],
})
export class RecipesTableComponent implements OnInit {
  recipes = [] as Recipe[];
  selectedRecipe: ExtendedRecipe | null = null;
  currentPage = 0;
  pageSize = 20;
  authorRecipes = [] as Recipe[];
  selectedAuthor = '';
  searchQuery = '';
  ingredientQuery: string = '';

  constructor(private recipesService: RecipesService) { }

  ngOnInit(): void {
    this.loadRecipes();
  }

  loadRecipes(): void {
    this.recipesService.getRecipes(this.currentPage, this.pageSize).subscribe(data => {
      console.log(data);
      this.recipes = data;
    });
  }

  changePage(newPage: number): void {
    this.currentPage = newPage;
    this.loadRecipes();
  }

  onSelectRecipe(id: string): void {
    //console.log('Clicked recipe ID:', id);
    this.recipesService.getRecipeById(id).subscribe({
      next: (data) => {
        //console.log('Loaded recipe details:', data);
        this.selectedRecipe = data;
      },
      error: (error) => {
        console.error('Failed to load recipe details:', error);
      }
    });
  }
  
  onSelectAuthor(authorName: string): void {
    this.selectedAuthor = authorName;
    this.recipesService.getRecipesByAuthor(authorName).subscribe({
      next: (data) => {
        this.authorRecipes = data;
      },
      error: (error) => {
        console.error('Failed to load recipes for author:', error);
      }
    });
  }

  searchRecipes(query: string): void {
    this.recipesService.getRecipesByName(query).subscribe(recipes => {
      this.recipes = recipes;
    }, error => {
      console.error('Failed to find recipes', error);
    });
  }

  onSearchChange(newQuery: string): void {
    this.searchQuery = newQuery;
    this.loadRecipes();
  }

  filterRecipesByIngredients(): void {
    const ingredients = this.ingredientQuery.split(',').map(ingredient => ingredient.trim());
    this.recipesService.getRecipesByIngredients(ingredients).subscribe(recipes => {
      this.recipes = recipes;
    }, error => {
      console.error('Failed to load recipes by ingredients', error);
    });
  }
  
}

