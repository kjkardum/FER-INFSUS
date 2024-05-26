
FROM node:20.9-buster-slim

WORKDIR /app

COPY package.json package-lock.json ./
COPY openapitools.json ./
COPY tsconfig.json ./
COPY next.config.mjs ./
COPY .eslintrc.json ./
COPY public ./public
COPY src ./src

RUN npm
RUN npm build
CMD ["npm", "start"]