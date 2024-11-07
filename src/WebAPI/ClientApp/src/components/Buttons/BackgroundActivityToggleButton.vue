<template>
	<IconButton
		size="md"
		color="default"
		icon-align="left"
		@click="toggleAnimationMode">
		<div>
			<q-badge
				color="red"
				floating
				rounded
				transparent>
				4
			</q-badge>
			<div class="activity-icon-container">
				<QSpinnerOval
					v-if="active"
					class="spinner"
					color="primary"
					:size="`${size + 2}px`" />
				<svg
					class="activity-icon"
					fill="currentColor"
					:viewBox="`0 0 48 48`"
					xmlns="http://www.w3.org/2000/svg">
					<path
						clip-rule="evenodd"
						d="M30.223 7.50024C30.8383 7.48916 31.3979 7.85502 31.6346 8.42308L37.5 22.5H44V25.5H36.5C35.8944 25.5 35.3483 25.1359 35.1154 24.5769L30.3234 13.0762L20.4045 39.5267C20.195 40.0853 19.6744 40.4667 19.0786 40.4979C18.4828 40.5292 17.9252 40.2044 17.6584 39.6708L10.5729 25.5H4V22.5H11.5C12.0682 22.5 12.5876 22.821 12.8416 23.3292L18.8033 35.2525L28.8455 8.47332C29.0616 7.8971 29.6077 7.51133 30.223 7.50024Z"
						fill="currentColor"
						fill-rule="evenodd" />

				</svg>
			</div>
		</div>
	</IconButton>
</template>

<script setup lang="ts">
const size = 32;

const active = ref(true);
const loading = ref(false);

function toggleAnimationMode(): void {
	active.value = !active.value;
	loading.value = !loading.value;
}
</script>

<style lang="scss">
.activity-icon-container {
  position: absolute;
  top: 0;
  left: 0;

  .spinner {
    position: absolute;
    top: 0.25rem;
    left: 0.25rem;
  }

  .activity-icon {
    position: absolute;
    width: 1.5rem;
    height: 1.5rem;
    top: 0.5rem;
    left: 0.5rem;
  }
}
</style>
