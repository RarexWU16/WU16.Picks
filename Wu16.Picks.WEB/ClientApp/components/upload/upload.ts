import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Category } from '../../interfaces'
import { getCategories } from '../../Services/http';
import { guid } from '../../Services/general';

@Component
export default class UploadComponent extends Vue {
    public categories: Category[] = [];
    public category: Category;
    public uploadText: string = '';
    public isNewCategory: boolean = false;
    public isLoading: boolean = true;

    public save(event: Event) {
        event.preventDefault();

        const data = new FormData(event.currentTarget as HTMLFormElement);
        data.append("CategoryName", this.category.name);

        if (this.category.id != null)
            data.append("CategoryId", this.category.id as string);

        this.isLoading = true;

        fetch("/api/upload-image", {
            method: "POST",
            body: data
        }).then(_ => {
            this.isLoading = false;
            this.$router.push('/');
        });
    }

    public onFileChange(event: Event) {
        this.uploadText = ((event.currentTarget as any).files[0] as File).name;
    }

    public toggleNewCategory(event: Event) {
        event.preventDefault();

        this.isNewCategory = !this.isNewCategory;

        if (this.isNewCategory) {
            this.category = {
                id: null,
                name: ""
            };
        }
        else {
            this.category = this.categories[0];
        }
    }

    mounted() {
        getCategories(categories => {
            this.categories = categories;

            if (this.categories.length != 0) {
                this.category = categories[0];
            }
            else {
                this.isNewCategory = true;
                this.category = {
                    id: guid(),
                    name: ""
                };
            }

            this.isLoading = false;
        });
    }
}
