import { Category, Image } from "../interfaces";

export type categoryCallback = (categories: Category[]) => void;
export type imagesCallback = (images: Image[]) => void;
export type basketCallback = (isInBasket: boolean) => void;

export const getCategories = (callback: categoryCallback): void => {
    fetch('api/categories', {
        credentials: 'include',
        method: 'GET',
    })
        .then(response => response.json() as Promise<Category[]>)
        .then(data => {
            callback(data);
        });
}

export const getBasket = (callback: imagesCallback): void => {
    fetch('api/basket', {
        credentials: 'include',
        method: 'GET',
    })
        .then(response => response.json() as Promise<Image[]>)
        .then(data => {
            callback(data);
        });
}

export const getImages = (categoryId: string, page: number, callback: imagesCallback): void => {
    let url: string = 'api/images/' + page;

    if (categoryId != null && categoryId != undefined && categoryId != "")
        url += `/${categoryId}`;

    fetch(url, {
        credentials: 'include',
        method: 'GET',
    })
        .then(response => response.json() as Promise<Image[]>)
        .then(data => {
            callback(data);
        });
}

export const toggleBasket = (imgId: string, callback: basketCallback) => {
    fetch("api/basket/toggle/" + imgId, {
        credentials: 'include',
        method: 'POST',
    })
        .then(response => response.json() as Promise<boolean>)
        .then(data => {
            callback(data);
        });
}