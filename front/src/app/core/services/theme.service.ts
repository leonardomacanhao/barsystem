import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'barsystem-theme';
  private isBrowser: boolean;
  
  constructor(
    @Inject(DOCUMENT) private document: Document,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
    if (this.isBrowser) {
      this.loadTheme();
    }
  }

  private loadTheme(): void {
    const savedTheme = localStorage.getItem(this.THEME_KEY);
    if (savedTheme === 'dark') {
      this.setDarkMode(true);
    }
  }

  toggleTheme(): void {
    const isDark = this.document.body.classList.contains('dark-theme');
    this.setDarkMode(!isDark);
    if (this.isBrowser) {
      localStorage.setItem(this.THEME_KEY, !isDark ? 'dark' : 'light');
    }
  }

  isDarkMode(): boolean {
    return this.document.body.classList.contains('dark-theme');
  }

  private setDarkMode(isDark: boolean): void {
    if (isDark) {
      this.document.body.classList.add('dark-theme');
    } else {
      this.document.body.classList.remove('dark-theme');
    }
  }
}
