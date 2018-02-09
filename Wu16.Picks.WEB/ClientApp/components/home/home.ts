import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Category, Image } from '../../interfaces'
import { getCategories, getImages } from '../../Services/http';

@Component({
    components: {
        gallery: require("../gallery/gallery.vue.html"),
        MugenScroll: require('vue-mugen-scroll')
    }
})
export default class HomeComponent extends Vue {
    public categories: Category[] = [];
    public images: Image[] = [];
    public isLoading: boolean = true;
    public isAtEnd: boolean = false;
    public category: string = "";

    private page: number = 0;

    get shouldLoad(): boolean {
        return !this.isLoading;
    }

    public fetchData() {
        this.isLoading = true;
        this.page += 1;

        getImages(this.category, this.page, this.pushImages);
    }

    public pushImages(images) {
        this.images = [...this.images, ...images];

        if (images.length != 0) setTimeout(_ => this.isLoading = false, 200);
        else { this.isLoading = false; }

        if (images.length < 9) {
            this.isAtEnd = true;
        }
    }

    public onChange() {
        this.page = -1;
        this.images = [];
        this.isLoading = true;
        this.isAtEnd = false;

        this.fetchData();
    }

    constructor() {
        super();

        this.fetchData = this.fetchData.bind(this);
        this.pushImages = this.pushImages.bind(this);
    }

    mounted() {
        getCategories(categories => this.categories = categories);
        getImages("", 0, this.pushImages);
    }
}
