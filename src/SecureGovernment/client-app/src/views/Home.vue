<template>
  <div class="home">
    <ScanRequest msg="How are you?"/>
  </div>
</template>

<script>
// @ is an alias to /src
import ScanRequest from '@/components/scan-request/ScanRequest.vue'

export default {
  name: 'home',
  components: {
    ScanRequest
  }
}
</script>
