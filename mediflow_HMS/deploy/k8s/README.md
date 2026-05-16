# Kubernetes deployment quickstart

## Staging
kubectl apply -k deploy/k8s/overlays/staging

## Production
kubectl apply -k deploy/k8s/overlays/production

Replace secret values in each overlay before apply. For enterprise use, move to External Secrets / Vault-backed secret sync.
