import Vue from 'vue';
import { Component, Prop, Inject } from 'vue-property-decorator';
import { Image } from '../../interfaces';
import { getBasket, toggleBasket } from '../../Services/http';
import { url } from '../../Services/general';

@Component
export default class BasketComponent extends Vue {
    public images: Image[] = [];
    public isLoading: boolean = true;

    public toggleBasket(id: string) {
        toggleBasket(id, result => {
            if (!result) {
                const index = this.images
                    .map(x => x.id)
                    .indexOf(id);

                if (index != -1)
                    this.images.splice(index, 1);
            }
        });
    }

    public url = url;

    mounted() {
        getBasket(images => {
            this.images = images;
            this.isLoading = false;
        });
    }
}
