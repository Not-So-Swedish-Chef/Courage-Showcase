# Frontend - Angular built and served with Nginx
# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
COPY angular.json ./
COPY tsconfig*.json ./
RUN npm ci

# Copy source and build with production configuration
COPY src ./src
COPY server.ts ./
RUN npm run build -- --configuration production

# Production stage
FROM nginx:alpine AS prod
# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copy built Angular app (browser) to Nginx html root
COPY --from=build /app/dist/events-hub-angular/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

