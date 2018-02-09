
export interface Category {
    name: string;
    id: string | null;
}

export interface Image {
    id: string;
    fileName: string;
    ratio: number;
    isInBasket: boolean;

    categoryName: string;
    categoryId: string;
}