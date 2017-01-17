//
//  FFIAPManager.m
//  FlowFree
//
//  Created by Tran Truong Giang on 8/4/14.
//  Copyright (c) 2014 Apportable. All rights reserved.
//

#import "FFIAPManager.h"
#import "MBProgressHUD.h"
@interface FFIAPManager () <RMStoreObserver>

@end

@implementation FFIAPManager

+ (instancetype)shareInstance {
    static id _instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[self alloc] init];
    });
    return _instance;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
        _persistence = [[RMStoreUserDefaultsPersistence alloc] init];
        _store = [[RMStore alloc] init];
        _store.transactionPersistor = _persistence;
        [_store addStoreObserver:self];
    }
    return self;
}

- (void)restoreTransactionsOnSuccess:(void (^)())successBlock
                             failure:(void (^)(NSError *error))failureBlock {
    __weak id self_weak = self;
    [self showProgressHUD];
    [_store restoreTransactionsOnSuccess:^(NSArray *transactions) {
        if (transactions.count != 0)
            if (successBlock)
                successBlock();
        [self_weak hideProgressHUD];
    } failure:^(NSError *error) {
        if (failureBlock)
            failureBlock(error);
        [self_weak hideProgressHUD];
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Purchase Error"
                                                        message:@"Something wrong happened. Try again."
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        [alert show];
    }];
}
- (void)addPayment:(NSString*)productIdentifier
           success:(void (^)(SKPaymentTransaction *transaction))successBlock
           failure:(void (^)(SKPaymentTransaction *transaction, NSError *error))failureBlock {
    
    SKProduct *product = [_store productForIdentifier:productIdentifier];
    __weak id self_weak = self;
    if (product != nil) {
        [self showProgressHUD];
        [self _startPurchasePayment:productIdentifier success:successBlock failure:failureBlock];
    } else {
        [self showProgressHUD];
        [_store requestProducts:[NSSet setWithObject:productIdentifier] success:^(NSArray *products, NSArray *invalidProductIdentifiers) {
            [self_weak _startPurchasePayment:productIdentifier success:successBlock failure:failureBlock];
        } failure:^(NSError *error) {
            if (failureBlock) {
                failureBlock(nil,error);
            }
            [self_weak hideProgressHUD];
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Purchase Error"
                                                            message:@"Something wrong happened. Try again."
                                                           delegate:nil
                                                  cancelButtonTitle:@"OK"
                                                  otherButtonTitles:nil];
            [alert show];
        }];
    }
}

- (BOOL)didPurchaseAnyProduct {
    return _persistence.purchasedProductIdentifiers.count != 0;
}

- (void)_startPurchasePayment:(NSString*)productIdentifier
                      success:(void (^)(SKPaymentTransaction *transaction))successBlock
                      failure:(void (^)(SKPaymentTransaction *transaction, NSError *error))failureBlock {
    __weak id self_weak = self;
    [_store addPayment:productIdentifier success:^(SKPaymentTransaction *transaction) {
        successBlock(transaction);
        [self_weak hideProgressHUD];
    } failure:^(SKPaymentTransaction *transaction, NSError *error) {
        if (failureBlock) {
            failureBlock(transaction,error);
        }
        [self_weak hideProgressHUD];
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Purchase Error"
                                                        message:@"Something wrong happened. Try again."
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        [alert show];
    }];
}

- (void)showProgressHUD {
    [MBProgressHUD showHUDAddedTo:[[UIApplication sharedApplication].delegate window] animated:YES];
}

- (void)hideProgressHUD {
    [MBProgressHUD hideHUDForView:[[UIApplication sharedApplication].delegate window] animated:YES];
}

#pragma mark -

- (void)storePaymentTransactionFinished:(id)sender {
    
}

@end
