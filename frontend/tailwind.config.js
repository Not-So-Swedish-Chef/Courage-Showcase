/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#3f51b5', // Example primary color matching Angular Material default
        secondary: '#ff4081',
      }
    },
  },
  plugins: [],
}

