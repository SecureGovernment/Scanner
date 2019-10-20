import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class ScanRequest extends Vue {
  @Prop() private domain!: string;

  submit (): void {
    fetch('/scan?domain=' + this.domain)
  }
}
