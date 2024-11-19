<template>
	<div class="help-group">
		<slot />
	</div>
</template>

<style lang="scss">
.help-group {
  display: grid;
  grid-template-columns: max-content min-content 1fr;
  gap: 10px;
  padding-right: 1rem;
}
</style>
